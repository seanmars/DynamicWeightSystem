/**
 * plugins/vuetify.ts
 *
 * Framework documentation: https://vuetifyjs.com`
 */

// Styles
import '@fortawesome/fontawesome-free/css/all.css';
import 'vuetify/styles';

// Composables
import { createVuetify } from 'vuetify';
import { aliases, fa } from 'vuetify/iconsets/fa';
import { zhHant, en } from 'vuetify/locale';
import DayJsAdapter from '@date-io/dayjs';
import dayjsLang from 'dayjs/locale/zh-tw';

// https://vuetifyjs.com/en/introduction/why-vuetify/#feature-guides
export default createVuetify({
  theme: {
    defaultTheme: 'light',
  },
  icons: {
    defaultSet: 'fa',
    aliases,
    sets: {
      fa,
    },
  },
  date: {
    adapter: DayJsAdapter,
    locale: {
      'zh-tw': dayjsLang,
    },
  },
  locale: {
    locale: 'zhHant',
    fallback: 'en',
    messages: { zhHant, en },
  },
});
