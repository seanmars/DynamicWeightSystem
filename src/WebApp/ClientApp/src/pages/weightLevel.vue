<template>
  <v-card width="600px">
    <v-card-title>
      分規資訊管理
    </v-card-title>

    <v-card-actions class="justify-end">
      <v-btn variant="outlined" color="purple" border @click="syncDataFormDevice">
        載入設備分規資訊
      </v-btn>

      <v-btn variant="outlined" color="blue" border @click="syncDataFormServer">
        載入伺服器資訊
      </v-btn>
    </v-card-actions>

    <v-divider />

    <div class="mt-3 ml-3">
      <WeightLevelList v-model="weightLevels" />
    </div>

    <v-card-actions class="d-flex justify-center mb-6">
      <v-spacer />
      <v-spacer />
      <v-spacer />
      <v-btn variant="outlined" color="primary" border @click="save">
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
import { useAppStore } from '@/stores/app';

const appStore = useAppStore();

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
  appStore.activateOverlay();
  try {
    const { isFetching, error, data } = await useFetch<WeightLevelList>('/api/weight-level')
      .json();

    if (error.value) {
      console.error(error.value);
      return;
    }

    weightLevels.value = data.value || [];
    originValues.value = _.cloneDeep(data.value);
  } catch (e) {
    console.error(e);
  } finally {
    await appStore.deactivateOverlay();
  }
};

const syncDataFormServer = async () => {
  await fetchWeightLevels();
};

const save = async () => {
  appStore.activateOverlay();
  try {
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
  } finally {
    await appStore.deactivateOverlay();
  }
};

const syncDataFormDevice = async () => {
  appStore.activateOverlay();
  try {
    const url = '/api/weight-level-from-device';
    const { isFetching, error, data } = await useFetch<WeightLevelList>(url)
      .json();

    weightLevels.value = data.value || [];
    originValues.value = _.cloneDeep(data.value);
  } finally {
    await appStore.deactivateOverlay();
  }
};

onMounted(async () => {
  await fetchWeightLevels();
});
</script>

<style scoped>

</style>
