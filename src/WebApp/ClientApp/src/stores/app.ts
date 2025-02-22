// Utilities
import { defineStore } from 'pinia';

const delay = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms));

export const useAppStore = defineStore('app', {
  state: () => ({
    overlay: false,
  }),
  getters: {
    getOverlay(): boolean {
      return this.overlay;
    },
  },
  actions: {
    toggleOverlay() {
      this.overlay = !this.overlay;
    },
    activateOverlay() {
      this.overlay = true;
    },
    async deactivateOverlay() {
      await delay(100);
      this.overlay = false;
    },
  },
});
