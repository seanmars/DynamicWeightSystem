<template>
  <!-- @vue-skip -->
  <div>
    <v-btn border variant="outlined" color="primary" @click="createItem">
      新增
    </v-btn>
    <v-data-table
      :headers="headers"
      :items="fishData"
      :items-per-page="itemsPerPage"
      item-value="id"
      height="500px"
      fixed-header
    >
      <template v-slot:item.actions="{ item }">
        <v-icon class="me-2" size="small" color="blue" icon="fa fa-pen-to-square" @click="editItem(item)" />
        <v-icon class="me-2" size="small" color="red" icon="fa fa-trash" @click="deleteItem(item)" />
      </template>
    </v-data-table>

    <v-dialog v-model="dialogUpsertConfirm" max-width="500px" persistent>
      <v-card>
        <v-card-title>
          <span class="headline">魚種</span>
        </v-card-title>

        <v-card-text>
          <v-form ref="form-new">
            <v-text-field
              v-model="editedItem.fishCode"
              label="編號"
              :rules="[rules.required]"
            />
            <v-text-field
              v-model="editedItem.name"
              label="魚種"
              :rules="[rules.required]"
            />
          </v-form>
        </v-card-text>

        <v-card-actions class="mb-4 d-flex justify-end mr-4">
          <v-btn
            variant="outlined"
            color="red"
            @click="closeUpsertDialog"
          >
            取消
          </v-btn>
          <div class="mx-2" />
          <v-btn
            variant="outlined"
            color="blue"
            @click="upsertItemConfirmed"
          >
            確認
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="dialogDeleteConfirm" max-width="500px">
      <v-card>
        <v-card-title class="text-h5">
          確定要刪除資料嗎?
        </v-card-title>
        <v-card-actions>
          <v-spacer />
          <v-btn
            color="red"
            variant="outlined"
            @click="closeDeleteDialog"
          >
            取消
          </v-btn>
          <v-btn
            color="blue"
            variant="outlined"
            @click="deleteItemConfirmed"
          >
            確認
          </v-btn>
          <v-spacer />
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useFetch } from '@vueuse/core';
import type { FishData } from '@/models';
import { useAppStore } from '@/stores/app';
import type { VForm } from 'vuetify/components';

const headers = [
  {
    title: '編號',
    key: 'fishCode',
    align: 'start',
  },
  {
    title: '魚種',
    key: 'name',
    align: 'start',
  },
  {
    title: '操作',
    key: 'actions',
    align: 'center',
    sortable: false,
  },
] as const;

const appStore = useAppStore();

const fishData = ref<FishData[]>([]);
const dialogUpsertConfirm = ref(false);
const dialogDeleteConfirm = ref(false);
const editedItem = ref<FishData>({ id: '', fishCode: '', name: '' });
const defaultItem = { id: '', fishCode: '', name: '' };
const editState = ref<'edit' | 'create'>('create');

const formNew = useTemplateRef<VForm>('form-new');

const itemsPerPage = computed(() => {
  return fishData.value.length;
});

const rules = {
  required: (value: string) => !!value || '必填欄位',
};

const getFishData = async () => {
  const url = '/api/fish-data';
  const { data, error } = await useFetch(url).json();
  if (error.value) {
    console.error(error.value);
    return;
  }

  fishData.value = data.value || [];
};

const createRequest = async () => {
  const url = '/api/fish-data';
  const { error, data } = await useFetch(url, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(editedItem.value),
  });

  if (error) {
    // TODO: notify error
    return;
  }

  // TODO: notify success
};

const updateRequest = async () => {
  const url = `/api/fish-data/${editedItem.value.id}`;
  const { data, error } = await useFetch(url, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(editedItem.value),
  }).json();

  if (error.value) {
    // TODO: notify error
    return;
  }

  // TODO: notify success
};

const deleteRequest = async () => {
  const url = `/api/fish-data/${editedItem.value.id}`;
  const { error } = await useFetch(url, {
    method: 'DELETE',
  });

  if (error.value) {
    // TODO: notify error
    return;
  }

  // TODO: notify success
};

const createItem = () => {
  editedItem.value = { ...defaultItem };
  dialogUpsertConfirm.value = true;
  editState.value = 'create';
};

const editItem = (item: FishData) => {
  editedItem.value = { ...item };
  dialogUpsertConfirm.value = true;
  editState.value = 'edit';
};

const deleteItem = (item: FishData) => {
  editedItem.value = Object.assign({}, item);
  dialogDeleteConfirm.value = true;
};

const deleteItemConfirmed = async () => {
  await deleteRequest();
  await getFishData();

  closeDeleteDialog();
};

const closeDeleteDialog = () => {
  dialogDeleteConfirm.value = false;
};

const upsertItemConfirmed = async () => {
  if (editState.value === 'create') {
    const { valid } = await formNew.value!.validate();
    if (!valid) {
      return false;
    }

    await createRequest();
  } else {
    await updateRequest();
  }

  await getFishData();

  closeUpsertDialog();
};

const closeUpsertDialog = () => {
  dialogUpsertConfirm.value = false;
  nextTick(() => {
    editedItem.value = Object.assign({}, defaultItem);
  });
};

onMounted(async () => {
  appStore.activateOverlay();
  try {
    await getFishData();
  } finally {
    await appStore.deactivateOverlay();
  }
});
</script>

<style scoped>

</style>
