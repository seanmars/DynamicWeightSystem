<template>
  <v-card width="600px">
    <v-card-title>
      魚種編號管理
    </v-card-title>
    <v-divider />

    <v-card-item>
      <FishDataList />
    </v-card-item>
  </v-card>
</template>

<script setup lang="ts">
import FishDataList from '@/components/FishDataList.vue';
</script>

<style scoped>

</style>
