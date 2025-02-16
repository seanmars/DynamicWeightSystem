<template>
  <div class="chart-container">
    <Line
      :width="width"
      :height="height"
      :data="data"
      :options="options"
      :style="chartStyle"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { Line } from 'vue-chartjs';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  LineElement,
  CategoryScale,
  LinearScale,
  PointElement,
} from 'chart.js';

ChartJS.register(Title, Tooltip, Legend, LineElement, CategoryScale, LinearScale, PointElement);

interface Props {
  width?: number | null;
  height?: number | null;
  data: Record<string, any>;
  options?: Record<string, any>;
}

const props = defineProps<Props>();

const chartStyle = computed(() => ({
  height: '500px',
  width: '100%',
  position: 'relative',
}));
</script>

<style scoped>
.chart-container {
  position: relative;
  height: 100%;
  width: 100%;
}
</style>
