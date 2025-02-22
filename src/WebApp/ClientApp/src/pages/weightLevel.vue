<template>
  <v-card width="600px">
    <v-card-title>
      分規資訊管理
    </v-card-title>
    <v-divider />

    <div class="mt-3 ml-3">
      <WeightLevelList v-model="weightLevels" />
    </div>

    <v-card-actions class="d-flex justify-center">
      <v-spacer />
      <v-btn color="error" border @click="cancel">
        回復
      </v-btn>
      <v-spacer />
      <v-btn color="primary" border @click="save">
        儲存
      </v-btn>
      <v-spacer />
    </v-card-actions>
  </v-card>

  <v-snackbar v-model="snackbar" :timeout="2000" :color="messageState">
    {{ message }}
  </v-snackbar>
</template>

<script setup lang="ts">
import * as _ from 'lodash-es';
import { useFetch } from '@vueuse/core';
import type { WeightLevelList } from '@/models';

const snackbar = ref<boolean>(false);
const message = ref<string>('');
const messageState = ref<'success' | 'error'>();

const weightLevels = ref<WeightLevelList>([]);
const originValues = ref<WeightLevelList>([]);

const showMessage = (msg: string, error = false) => {
  message.value = msg;
  snackbar.value = true;

  if (error) {
    messageState.value = 'error';
  } else {
    messageState.value = 'success';
  }
};

const fetchWeightLevels = async () => {
  const { isFetching, error, data } = await useFetch<WeightLevelList>('/api/weight-level')
    .json();

  if (error.value) {
    console.error(error.value);
    return;
  }

  weightLevels.value = data.value || [];
  originValues.value = _.cloneDeep(data.value);
};

const cancel = () => {
  weightLevels.value = _.cloneDeep(originValues.value);
};

const save = async () => {
  const url = '/api/weight-level';
  const { isFetching, error, response } = await useFetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(weightLevels.value),
  }).json();

  if (error.value) {
    console.error(error.value);
    showMessage('儲存失敗');
    return;
  }

  originValues.value = _.cloneDeep(weightLevels.value);
  showMessage('儲存成功');
};

onMounted(async () => {
  await fetchWeightLevels();
});
</script>

<style scoped>

</style>
