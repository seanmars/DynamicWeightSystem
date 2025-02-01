<template>
  <div>
    <v-data-table
      :headers="headers"
      :items="fishWeightHistory"
      item-value="id">
      <template v-slot:item.timestamp="{ item }">
        {{ new Date(item.timestamp).toLocaleString() }}
      </template>
    </v-data-table>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useFetch } from '@vueuse/core';
import type { FishWeightHistory } from '@/models';

const headers = [
  { text: 'ID', value: 'id' },
  { text: 'Timestamp', value: 'timestamp' },
  { text: 'Fish Code', value: 'fishCode' },
  { text: 'Weight', value: 'weight' },
];

const fishWeightHistory = ref<FishWeightHistory[]>([]);

const getFishWeightHistory = async () => {
  const url = '/api/fish-weight-history';
  const { data, error } = await useFetch(url).json<FishWeightHistory[]>();
  if (error.value) {
    console.error(error.value);
    return;
  }
  fishWeightHistory.value = data.value || [];
};

onMounted(async () => {
  await getFishWeightHistory();
});
</script>

<style scoped>

</style>
