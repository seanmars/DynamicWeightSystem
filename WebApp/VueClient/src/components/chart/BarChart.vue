<template>
  <div class="chart-container">
    <Bar
      :width="width"
      :height="height"
      :data="data"
      :options="options"
      :style="chartStyle"
    />
  </div>
</template>

<script>
import { Bar } from 'vue-chartjs';
import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale } from 'chart.js';

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale);

export default {
  name: 'BarChart',
  components: { Bar },
  props: {
    width: {
      type: Number,
      default: null,
    },
    height: {
      type: Number,
      default: null,
    },
    data: {
      type: Object,
      required: true,
    },
    options: {
      type: Object,
      default: () => {
        return {
          responsive: false,
        };
      },
    },
  },
  computed: {
    chartStyle() {
      return {
        height: '500px',
        width: '100%',
        position: 'relative',
      };
    },
  },
};
</script>

<style scoped>
.chart-container {
  position: relative;
  height: 100%;
  width: 100%;
}
</style>
