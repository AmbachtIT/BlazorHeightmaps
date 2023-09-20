/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './src/**/*.html',
    './src/**/*.razor',
  ],
  purge: {
    enabled: true,
    content: [
      './src/**/*.html',
      './src/**/*.razor'
    ],
  },
  theme: {
    extend: {},
  },
  plugins: [],
}

