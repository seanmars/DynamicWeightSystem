<template>
  <v-list>
    <template
      v-for="(item, index) in items"
      :key="index"
    >
      <v-list-item
        :prepend-icon="item.icon"
        :to="item.to"
      >
        {{ item.text }}
      </v-list-item>
    </template>
  </v-list>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import routeSet, { type ItemLink } from '@/router/routeSet';

const items = ref<ItemLink[]>(routeSet);
</script>

<style scoped>

</style>
