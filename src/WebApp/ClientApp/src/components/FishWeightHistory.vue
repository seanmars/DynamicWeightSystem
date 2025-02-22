<template>
  <v-card class="top-container">
    <v-card-actions>
      <v-btn variant="outlined" border color="blue" @click="setDateRangeToToday">
        當日
      </v-btn>
      <v-btn variant="outlined" border color="orange" @click="setDateRangeToThisMonth">
        本月
      </v-btn>
    </v-card-actions>

    <v-card-actions>
      <span>日期範圍</span>
      <DatePicker
        v-model.range="range"
        locale="zh-TW"
        color="blue"
      >
        <template #default="{ inputValue, inputEvents }">
          <v-row align="center" justify="center">
            <v-col>
              <v-text-field
                :value="inputValue.start"
                class="center-text pt-5"
                type="text"
                variant="solo"
                density="compact"
                readonly
                v-on="inputEvents.start"
              />
            </v-col>
            <v-col cols="1" class="align-center justify-center">
              <v-icon icon="fa-solid fa-arrow-right" />
            </v-col>
            <v-col>
              <v-text-field
                :value="inputValue.end"
                class="center-text pt-5"
                type="text"
                variant="solo"
                density="compact"
                readonly
                v-on="inputEvents.end"
              />
            </v-col>
          </v-row>
        </template>
      </DatePicker>
    </v-card-actions>

    <v-card-actions>
      <v-combobox
        v-model="selectedFish"
        label="魚種 (若不選則查詢全部)"
        variant="outlined"
        :items="fishData"
        item-title="name"
        item-value="fishCode"
        multiple
        clearable
      >
      </v-combobox>
    </v-card-actions>

    <v-card-actions>
      <v-btn variant="outlined" border block color="primary" @click="getFishWeightHistory">
        查詢
      </v-btn>
    </v-card-actions>

    <v-card-item>
      <v-data-table-virtual
        :headers="headers"
        :items="datasetByFish"
        fixed-header
        height="350px"
        item-value="id"
      >
        <template v-slot:item.fish="{ item }">
          {{ getFishName(item.fish) }}({{ item.fish }})
        </template>
        <template v-slot:item.dataset="{ item }">
          <v-btn variant="outlined" border text="分規圖表" @click="showWeightLevelChart(item)" />
          <span class="ma-1" />
          <v-btn variant="outlined" border text="歷史圖表" @click="showLineChart(item)" />
        </template>
      </v-data-table-virtual>
    </v-card-item>
  </v-card>

  <div class="mt-5">
    <v-card>
      <v-card-title>
        圖表
      </v-card-title>
      <v-card-item>
        <div v-if="chartType == 'bar'">
          <BarChart
            v-if="!!chartData"
            :data="chartData"
          />
        </div>
        <div v-else-if="chartType == 'line'">
          <LineChart
            v-if="!!chartData"
            :data="chartData"
            :options="{ interaction: { intersect: false, mode: 'index', axis: 'x' } }"
          />
        </div>
      </v-card-item>
    </v-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useFetch } from '@vueuse/core';
import dayjs from 'dayjs';
import * as _ from 'lodash-es';
import { DatePicker } from 'v-calendar';
import BarChart from '@/components/chart/BarChart.vue';
import LineChart from '@/components/chart/LineChart.vue';
import type { FishData, FishWeightHistory, WeightLevelList } from '@/models';
import { useAppStore } from '@/stores/app';

type ChartType = 'null' | 'bar' | 'line';

const appStore = useAppStore();

const headers = [
  { title: '魚種', key: 'fish', sortable: false },
  { title: '重量(平均)', key: 'weight', sortable: false },
  { title: '圖表', key: 'dataset', sortable: false },
];

const chartType = ref<ChartType>('null');

const selectedFish = ref<FishData[] | null>(null);

const range = ref({
  start: dayjs().toDate(),
  end: dayjs().toDate(),
});

interface chartDataType {
  labels: string[] | string[][];
  datasets: {
    label: string;
    data: number[];
    backgroundColor: string[];
    borderColor: string[];
    borderWidth: number
    pointBackgroundColor?: string;
    pointStyle?: string | boolean;
    maxBarThickness?: number;
  }[];
}

const weightLevels = ref<WeightLevelList>([]);

const chartData = ref<chartDataType | null>(null);
const fishData = ref<FishData[]>([]);
const fishWeightHistory = ref<FishWeightHistory[]>([]);

const getFishData = async () => {
  const url = '/api/fish-data';
  const { data, error } = await useFetch(url).json();
  if (error.value) {
    console.error(error.value);
    return;
  }

  fishData.value = data.value || [];
};

const getFishWeightHistory = async () => {
    let url = '/api/fish-sampling';

    const start = dayjs(range.value.start)
      .startOf('day')
      .unix();
    const end = dayjs(range.value.end)
      .add(1, 'day')
      .startOf('day')
      .unix();

    const params = new URLSearchParams();
    params.append('start', start.toString());
    params.append('end', end.toString());

    if (!!selectedFish.value) {
      for (const f of selectedFish.value) {
        params.append('fish', f.fishCode);
      }
    }

    url = `${url}?${params.toString()}`;

    const { data, error } = await useFetch(url)
      .get()
      .json<FishWeightHistory[]>();
    if (error.value) {
      console.error(error.value);
      return;
    }
    fishWeightHistory.value = data.value || [];

    console.log(fishWeightHistory.value.length);
  }
;

const getFishName = (fishCode: string) => {
  const fish = fishData.value.find((item) => item.fishCode === fishCode);
  return fish ? fish.name : fishCode;
};

const calculateWeightRange = (item: FishWeightHistory[], levels: WeightLevelList): Record<string, {
  level: number,
  data: FishWeightHistory
}[]> => {
  if (item.length === 0) {
    return {};
  }

  const fishGroup: Record<string, FishWeightHistory[]> = item.reduce((acc: any, item) => {
    if (!acc[item.fishCode]) {
      acc[item.fishCode] = [];
    }
    acc[item.fishCode].push(item);
    return acc;
  }, {});

  const rangeFish: Record<string, {
    level: number,
    data: FishWeightHistory
  }[]> = {};

  for (const fishCode in fishGroup) {
    const fishData = fishGroup[fishCode];

    for (const fishDataKey in fishData) {
      const data = fishData[fishDataKey];
      const level = levels.find((item) => {
        return data.weight >= item.lowerBound && data.weight <= item.upperBound;
      });

      if (!level) {
        console.log(`No level found for ${data.weight} g`);
        continue;
      }

      const key = `${fishCode}-${level.level}`;
      if (!rangeFish[key]) {
        rangeFish[key] = [];
      }
      rangeFish[key].push({ level: level.level, data });
    }

  }

  return rangeFish;
};

const datasetByFish = computed(() => {
  if (fishWeightHistory.value.length === 0) {
    return [];
  }

  // Group by fish code
  let fishGroup = fishWeightHistory.value.reduce((acc: any, item) => {
    if (!acc[item.fishCode]) {
      acc[item.fishCode] = [];
    }
    acc[item.fishCode].push(item);
    return acc;
  }, {});

  // Order by fish code
  fishGroup = Object.keys(fishGroup).sort().reduce((acc: any, key) => {
    acc[key] = fishGroup[key];
    return acc;
  }, {});

  return Object.keys(fishGroup).map((fishCode) => {
    const fishData = fishGroup[fishCode];
    const totalWeight = fishData.reduce((acc: number, item: FishWeightHistory) => {
      return acc + item.weight;
    }, 0);

    // Calculate average weight and cut to 2 decimal places
    const avgWeight = (totalWeight / fishData.length).toFixed(2);

    return {
      fish: fishCode,
      weight: `${avgWeight} g`,
      dataset: fishData,
    };
  });
});

const showLineChart = (item: any) => {
  chartType.value = 'line';

  const labels = item.dataset.map((data: FishWeightHistory) => {
    return dayjs.unix(data.timestamp).format('YYYY-MM-DD HH:mm:ss');
  });

  const data = item.dataset.map((data: FishWeightHistory) => {
    return data.weight;
  });

  const avgWeight = data.reduce((acc: number, item: number) => {
    return acc + item;
  }, 0) / data.length;
  const avgData = Array(data.length).fill(avgWeight);

  chartData.value = {
    labels,
    datasets: [
      {
        label: getFishName(item.fish),
        data,
        backgroundColor: ['rgba(93,171,224,0.2)'],
        borderColor: ['rgb(65,103,231)'],
        borderWidth: 1,
        pointBackgroundColor: 'rgb(15,50,162)',
        pointStyle: 'circle',
      },
      {
        label: `平均(${avgWeight.toFixed(2)} g)`,
        data: avgData,
        backgroundColor: ['rgba(255, 99, 132, 0.2)'],
        borderColor: ['rgb(255, 99, 132)'],
        borderWidth: 1,
        pointBackgroundColor: 'rgb(255, 99, 132)',
        pointStyle: false,
      },
    ],
  };
};

const showWeightLevelChart = (item: any) => {
  chartType.value = 'bar';

  const rangeFish = calculateWeightRange(item.dataset, weightLevels.value || []);
  const flatRangeFish = _.sortBy(Object.keys(rangeFish).map((key) => {
    return {
      key,
      data: rangeFish[key],
    };
  }), s => s.data[0].level);

  const labels = weightLevels.value.map((item) => {
    return [`${item.level}`, `${item.lowerBound} ~ ${item.upperBound} g`];
  });

  const datasets: number[] = [];
  for (const level of weightLevels.value) {
    const fishData = flatRangeFish.find((item) => {
      if (item.data[0].level === level.level) {
        datasets.push(item.data.length);
        return true;
      }
      return false;
    });

    datasets.push(!!fishData ? fishData?.data.length : 0);
  }

  chartData.value = {
    labels,
    datasets: [
      {
        label: getFishName(item.fish),
        data: datasets,
        backgroundColor: ['rgba(93,171,224,0.2)'],
        borderColor: ['rgb(65,103,231)'],
        borderWidth: 1,
        maxBarThickness: 100,
      },
    ],
  };
};

const fetchWeightLevels = async () => {
  const { isFetching, error, data } = await useFetch<WeightLevelList>('/api/weight-level')
    .json();

  if (error.value) {
    console.error(error.value);
    return;
  }

  weightLevels.value = data.value || [];
};

const setDateRangeToToday = () => {
  range.value = {
    start: dayjs().toDate(),
    end: dayjs().toDate(),
  };
};

const setDateRangeToThisMonth = () => {
  range.value = {
    start: dayjs().startOf('month').toDate(),
    end: dayjs().endOf('month').toDate(),
  };
};

onMounted(async () => {
  appStore.activateOverlay();

  try {
    await fetchWeightLevels();
    await getFishData();
  } finally {
    await appStore.deactivateOverlay();
  }
});
;
</script>

<style scoped>
.top-container {
  height: 500px;
  width: 800px;
}

.center-text :deep(input) {
  text-align: center;
}
</style>
