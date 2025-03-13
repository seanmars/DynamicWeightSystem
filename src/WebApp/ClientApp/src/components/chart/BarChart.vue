<template>
  <div class="chart-container">
    <!-- @vue-skip -->
    <Bar
      :width="width"
      :height="height"
      :data="data"
      :options="mergedOptions"
      :style="chartStyle"
      :plugins="plugins"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { Bar } from 'vue-chartjs';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale
} from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale);

interface Props {
  width?: number | null;
  height?: number | null;
  data: Record<string, any>;
  total?: number;
  options?: Record<string, any>;
}

const props = defineProps<Props>();

const plugins = [ChartDataLabels];

const mergedOptions = computed(() => {
  return {
    ...defaultOptions,
    ...props.options
  };
});

const defaultOptions = {
  plugins: {
    title: {
      display: !!props.total,
      text: () => {
        return `總數: ${props.total ?? 0}`;
      },
      font: {
        size: 16
      }
    },
    datalabels: {
      formatter: (value: any, context: any) => {
        if (value === 0) return '';

        const dataset = context.chart.data.datasets[0];
        const total = dataset.data.reduce((sum: any, val: any) => sum + val, 0);
        // percentage with two decimal points
        const percentage = ((value / total) * 100).toFixed(2);
        return `${value}\n(${percentage}%)`;
      },
      anchor: 'end',
      align: 'start',
      textAlign: 'center',
      offset: 10,
      font: {
        size: 16,
      }
    },
  },
  scales: {
    y: {
      beginAtZero: true,
      ticks: {
        stepSize: 1,
      },
    }
  }
};

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
