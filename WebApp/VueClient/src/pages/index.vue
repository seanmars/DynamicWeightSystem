<template>
  <v-card>
    <BarChart
      :width="500"
      :height="500"
      :data="chartData"
    />
  </v-card>
</template>

<script lang="ts" setup>
import BarChart from '@/components/chart/BarChart';

const chartData = ref({
  labels: ['January', 'February', 'March'],
  datasets: [
    {
      label: 'My First Dataset',
      data: [40, 20, 12],
    },
  ],
});
</script>
