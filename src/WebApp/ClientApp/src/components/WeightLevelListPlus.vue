<template>
  <draggable
    v-model="weightLevels"
    v-bind="dragOptions"
    group="weight-level"
    item-key="level"
    handle=".handle"
    @start="startDrag"
    @end="endDrag"
  >
    <template #item="{element}">
      <v-row class="align-center">
        <v-col class="handle cursor-move" cols="1">
          <v-icon icon="fas fa-list" />
        </v-col>
        <v-col cols="1">
          <v-label>{{ element.level }}.</v-label>
        </v-col>
        <v-col cols="4" class="d-flex align-center">
          <v-text-field
            v-model="element.lowerBound"
            label="最小值"
            density="compact"
            variant="outlined"
            type="number"
          />
        </v-col>
        <v-col cols="4" class="d-flex align-center">
          <v-text-field
            v-model="element.upperBound"
            label="最大值"
            density="compact"
            variant="outlined"
            type="number"
          />
        </v-col>
      </v-row>
    </template>
  </draggable>
</template>

<script setup lang="ts">
import draggable from 'vuedraggable';
import type { WeightLevelList } from '@/models';

const weightLevels = defineModel<WeightLevelList>({
  default: []
});

const drag = ref<boolean>(false);
const dragOptions = ref({
  animation: 0,
  group: 'description',
  disabled: false,
  ghostClass: 'ghost'
});

const startDrag = (e: any) => {
  console.log(e);
  drag.value = true;
};

const endDrag = (e: any) => {
  console.log(e);
  drag.value = false;
  const len = weightLevels.value.length;
  for (let i = 0; i < len; i++) {
    weightLevels.value[i].level = i + 1;
  }
};
</script>

<style scoped>
.ghost {
  opacity: 0.5;
  background: #c8ebfb;
}
</style>
