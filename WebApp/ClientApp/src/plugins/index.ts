/**
 * plugins/index.ts
 *
 * Automatically included in `./src/main.ts`
 */

// Plugins
import vuetify from './vuetify';
import pinia from '../stores';
import router from '../router';

import { setupCalendar, Calendar, DatePicker } from 'v-calendar';
import 'v-calendar/style.css';

// Types
import type { App } from 'vue';

export function registerPlugins(app: App) {
  app
    .use(vuetify)
    .use(router)
    .use(pinia);

  // Use plugin defaults (optional)
  app.use(setupCalendar, {});

  // Use the components
  app.component('VCalendar', Calendar);
  app.component('VDatePicker', DatePicker);
}
