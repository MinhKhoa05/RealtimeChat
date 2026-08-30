const hasTailwindPostcss = await import('@tailwindcss/postcss')
  .then(() => true)
  .catch(() => false);

export default {
  plugins: hasTailwindPostcss
    ? {
        '@tailwindcss/postcss': {},
        autoprefixer: {},
      }
    : {
        tailwindcss: {},
        autoprefixer: {},
      },
};
