<template>
  <v-card>
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
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useFetch } from '@vueuse/core';
import dayjs from 'dayjs';
import { DatePicker } from 'v-calendar';

import type { FishWeightHistory } from '@/models';

const headers = [
  { title: '魚種', key: 'fish' },
  { title: '重量(平均)', key: 'weight' },
  { title: '圖表', key: 'dataset' },
];

const range = ref({
  start: dayjs().toDate(),
  end: dayjs().toDate(),
});

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
  text-align: center
}
</style>
