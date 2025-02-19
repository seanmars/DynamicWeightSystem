<template>
  <div v-for="weightLevel in weightLevels" :key="weightLevel.level">
    <v-row class="align-center">
      <v-col cols="1">
        <v-label>{{ weightLevel.level }}.</v-label>
      </v-col>
      <v-col cols="4" class="d-flex align-center">
        <v-text-field
          v-model="weightLevel.lowerBound"
          label="最小值"
          density="compact"
          variant="outlined"
          type="number"
        />
      </v-col>
      <v-col cols="4" class="d-flex align-center">
        <v-text-field
          v-model="weightLevel.upperBound"
          label="最大值"
          density="compact"
          variant="outlined"
          type="number"
        />
      </v-col>
    </v-row>
  </div>
</template>

<script setup lang="ts">
import type { WeightLevelList } from '@/models';

const weightLevels = defineModel<WeightLevelList>({
  default: []
});
</script>

<style scoped>

</style>
