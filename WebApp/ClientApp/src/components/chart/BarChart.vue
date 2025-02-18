<template>
  <div class="chart-container">
    <!-- @vue-skip -->
    <Bar
      :width="width"
      :height="height"
      :data="data"
      :options="options"
      :style="chartStyle"
    />
  </div>
</template>

<script setup lang="ts">
import { Bar } from 'vue-chartjs';
import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale } from 'chart.js';
import { computed } from 'vue';

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale);

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
