<template>
  <!-- @vue-skip -->
  <div>
    <v-btn color="primary" @click="createItem">
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

    <v-dialog v-model="dialogConfirm" max-width="500px">
      <v-card>
        <v-card-title>
          <span class="headline">魚種</span>
        </v-card-title>

        <v-card-text>
          <v-form ref="form">
            <v-text-field v-model="editedItem.fishCode" label="編號" />
            <v-text-field v-model="editedItem.name" label="魚種" />
          </v-form>
        </v-card-text>

        <v-card-actions>
          <v-spacer />
          <v-btn
            color="blue darken-1"
            @click="close"
          >
            取消
          </v-btn>
          <v-btn
            color="blue darken-1"
            @click="save"
          >
            確認
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="dialogDelete" max-width="500px">
      <v-card>
        <v-card-title class="text-h5">
          確定要刪除資料嗎?
        </v-card-title>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue-darken-1" variant="text" @click="closeDelete">
            取消
          </v-btn>
          <v-btn color="blue-darken-1" variant="text" @click="deleteItemConfirm">
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

const fishData = ref<FishData[]>([]);
const dialogConfirm = ref(false);
const dialogDelete = ref(false);
const editedItem = ref<FishData>({ id: '', fishCode: '', name: '' });
const defaultItem = { id: '', fishCode: '', name: '' };
const editState = ref<'edit' | 'create'>('create');

const itemsPerPage = computed(() => {
  return fishData.value.length;
});

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
  dialogConfirm.value = true;
  editState.value = 'create';
};

const editItem = (item: FishData) => {
  editedItem.value = { ...item };
  dialogConfirm.value = true;
  editState.value = 'edit';
};

const deleteItem = (item: FishData) => {
  editedItem.value = Object.assign({}, item);
  dialogDelete.value = true;
};

const deleteItemConfirm = async () => {
  await deleteRequest();
  await getFishData();

  dialogDelete.value = false;
  closeDelete();
};

const closeDelete = () => {
  close();
};

const save = async () => {
  if (editState.value === 'create') {
    await createRequest();
  } else {
    await updateRequest();
  }

  await getFishData();

  close();
};

const close = () => {
  dialogConfirm.value = false;
  nextTick(() => {
    editedItem.value = Object.assign({}, defaultItem);
  });
};

onMounted(async () => {
  await getFishData();
});
</script>

<style scoped>

</style>
