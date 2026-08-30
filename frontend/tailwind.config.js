/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    extend: {
      colors: {
        bg: 'var(--color-bg)',
        panel: 'var(--color-panel)',
        card: 'var(--color-card)',
        border: 'var(--color-border)',
        'border-subtle': 'var(--color-border-subtle)',
        text: 'var(--color-text)',
        soft: 'var(--color-text-soft)',
        muted: 'var(--color-text-muted)',
        disabled: 'var(--color-text-disabled)',
        primary: 'var(--color-primary)',
        'primary-soft': 'var(--color-primary-soft)',
        'primary-text': 'var(--color-primary-text)',
      },
      borderRadius: {
        'radius-md': 'var(--radius-md)',
        'radius-lg': 'var(--radius-lg)',
        'radius-xl': 'var(--radius-xl)',
        control: 'var(--radius-control)',
        action: 'var(--radius-action)',
        icon: 'var(--radius-icon)',
        item: 'var(--radius-item)',
      },
      boxShadow: {
        header: 'var(--shadow-header)',
        dialog: 'var(--shadow-dialog)',
        loading: 'var(--shadow-loading)',
        'media-active': 'var(--shadow-media-active)',
        'nav-active': 'var(--shadow-nav-active)',
        'primary-action': 'var(--shadow-primary-action)',
      },
    },
  },
  plugins: [],
};
