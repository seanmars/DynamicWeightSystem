<template>
  <v-card class="top-container">
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
          <v-btn variant="elevated" text="區間圖表" @click="showRangeChart(item)" />
          <span class="ma-1" />
          <v-btn variant="elevated" text="歷史圖表" @click="showLineChart(item)" />
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
import { sort as radashSort } from 'radash';
import { DatePicker } from 'v-calendar';
import BarChart from '@/components/chart/BarChart.vue';
import LineChart from '@/components/chart/LineChart.vue';

import type { FishData, FishWeightHistory } from '@/models';

type ChartType = 'null' | 'bar' | 'line';

const headers = [
  { title: '魚種', key: 'fish' },
  { title: '重量(平均)', key: 'weight' },
  { title: '圖表', key: 'dataset' },
];

const chartType = ref<ChartType>('null');

const range = ref({
  start: dayjs().toDate(),
  end: dayjs().toDate(),
});

interface chartDataType {
  labels: string[];
  datasets: {
    label: string;
    data: number[];
    backgroundColor: string[];
    borderColor: string[];
    borderWidth: number
    pointBackgroundColor?: string;
    pointStyle?: string | boolean;
  }[];
}

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
  const url = '/api/fish-sampling';
  const { data, error } = await useFetch(url).json<FishWeightHistory[]>();
  if (error.value) {
    console.error(error.value);
    return;
  }
  fishWeightHistory.value = data.value || [];
};

const getFishName = (fishCode: string) => {
  const fish = fishData.value.find((item) => item.fishCode === fishCode);
  return fish ? fish.name : fishCode;
};

const calculateWeightRange = (item: FishWeightHistory[], range: number = 200): Record<string, {
  weight: number,
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
    weight: number,
    data: FishWeightHistory
  }[]> = {};
  for (const fishCode in fishGroup) {
    const fishData = fishGroup[fishCode];

    // group by every range
    const rangeGroup: Record<string, {
      weight: number,
      data: FishWeightHistory
    }[]> = fishData.reduce((acc: any, data) => {
      const w = Math.floor(data.weight / range) * range;
      const key = `${fishCode}-${w} g`;
      if (!acc[key]) {
        acc[key] = [];
      }
      acc[key].push({ weight: w, data: data });
      return acc;
    }, {});

    for (const rangeGroupKey in rangeGroup) {
      if (!rangeFish[rangeGroupKey]) {
        rangeFish[rangeGroupKey] = [];
      }
      rangeFish[rangeGroupKey].push(...rangeGroup[rangeGroupKey]);
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

const showRangeChart = (item: any) => {
  chartType.value = 'bar';

  const rangeFish = calculateWeightRange(item.dataset, 50);
  const flatRangeFish = radashSort(Object.keys(rangeFish).map((key) => {
    return {
      key,
      data: rangeFish[key],
    };
  }), s => s.data[0].weight);

  const labels = flatRangeFish.map((item) => {
    return item.key.split('-')[1];
  });
  const datasets = flatRangeFish.map((item) => {
    return item.data.length;
  });

  chartData.value = {
    labels,
    datasets: [
      {
        label: getFishName(item.fish),
        data: datasets,
        backgroundColor: ['rgba(93,171,224,0.2)'],
        borderColor: ['rgb(65,103,231)'],
        borderWidth: 1,
      },
    ],
  };
};

onMounted(async () => {
  await getFishData();
  await getFishWeightHistory();
});
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
