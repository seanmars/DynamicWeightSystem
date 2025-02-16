<template>
  <div class="v-row">
    <v-card class="v-col-5">
      <v-card-actions>
        <v-card-title>
          <span>日期範圍</span>
        </v-card-title>
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
          height="600"
          item-value="id"
        >
          <template v-slot:item.dataset="{ item }">
            <v-btn text="顯示圖表" @click="showChart(item)" />
          </template>
        </v-data-table-virtual>
      </v-card-item>
    </v-card>
    <div class="pa-1" />
    <v-card class="v-col">
      <v-card-title>
        圖表
      </v-card-title>

    </v-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useFetch } from '@vueuse/core';
import dayjs from 'dayjs';
import { DatePicker } from 'v-calendar';

import type { FishWeightHistory } from '@/models';
import BarChart from '@/components/chart/BarChart';

const headers = [
  { title: '魚種', key: 'fish' },
  { title: '重量(平均)', key: 'weight' },
  { title: '圖表', key: 'dataset' },
];

const range = ref({
  start: dayjs().toDate(),
  end: dayjs().toDate(),
});

const labels = ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange', 'Grey'];
const data = {
  labels: labels,
  datasets: [{
    label: 'My First Dataset',
    data: [65, 59, 80, 81, 56, 55, 40],
    backgroundColor: [
      'rgba(255, 99, 132, 0.2)',
      'rgba(255, 159, 64, 0.2)',
      'rgba(255, 205, 86, 0.2)',
      'rgba(75, 192, 192, 0.2)',
      'rgba(54, 162, 235, 0.2)',
      'rgba(153, 102, 255, 0.2)',
      'rgba(201, 203, 207, 0.2)'
    ],
    borderColor: [
      'rgb(255, 99, 132)',
      'rgb(255, 159, 64)',
      'rgb(255, 205, 86)',
      'rgb(75, 192, 192)',
      'rgb(54, 162, 235)',
      'rgb(153, 102, 255)',
      'rgb(201, 203, 207)'
    ],
    borderWidth: 1
  }]
};

const fishWeightHistory = ref<FishWeightHistory[]>([]);

const getFishWeightHistory = async () => {
  const url = '/api/fish-sampling';
  const { data, error } = await useFetch(url).json<FishWeightHistory[]>();
  if (error.value) {
    console.error(error.value);
    return;
  }
  fishWeightHistory.value = data.value || [];
};

const dataset = computed(() => {
  if (fishWeightHistory.value.length === 0) {
    return [];
  }

  return fishWeightHistory.value.map((item) => {
    return {
      id: item.id,
      date: dayjs.unix(item.timestamp).format('YYYY-MM-DD HH:mm:ss'),
      fish: item.fishCode,
      weight: `${item.weight} g`,
    };
  });
});

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

const showChart = (item: any) => {
  console.log(item);
};

onMounted(async () => {
  await getFishWeightHistory();
});
</script>

<style scoped>
.center-text :deep(input) {
  text-align: center;
}
</style>
