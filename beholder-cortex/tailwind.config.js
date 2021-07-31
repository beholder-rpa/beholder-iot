/* eslint-disable @typescript-eslint/no-var-requires */
const defaultTheme = require('tailwindcss/defaultTheme');

function withOpacity(colorVariable) {
  return ({ opacity }) => {
    if (opacity !== undefined) {
      return `hsla(var(${colorVariable}), ${opacity})`;
    }
    return `hsl(var(${colorVariable}))`;
  };
}

module.exports = {
  mode: 'jit',
  purge: ['./public/**/*.html', './src/**/*.{js,ts,jsx,tsx}'],
  darkMode: 'media', // or 'media' or 'class'
  theme: {
    extend: {
      spacing: {
        96: '24rem',
      },
      fontFamily: {
        sans: ['Inter var', ...defaultTheme.fontFamily.sans],
      },
      textColor: {
        'skin-900': withOpacity('--skin-color-900'),
        'skin-800': withOpacity('--skin-color-800'),
        'skin-700': withOpacity('--skin-color-700'),
        'skin-600': withOpacity('--skin-color-600'),
        'skin-500': withOpacity('--skin-color-500'),
        'skin-400': withOpacity('--skin-color-400'),
        'skin-300': withOpacity('--skin-color-300'),
        'skin-200': withOpacity('--skin-color-200'),
        'skin-100': withOpacity('--skin-color-100'),
      },
      backgroundColor: {
        dark: '#1E1E1E',
        'skin-950': withOpacity('--skin-color-950'),
        'skin-900': withOpacity('--skin-color-900'),
        'skin-800': withOpacity('--skin-color-800'),
        'skin-700': withOpacity('--skin-color-700'),
        'skin-600': withOpacity('--skin-color-600'),
        'skin-500': withOpacity('--skin-color-500'),
        'skin-400': withOpacity('--skin-color-400'),
        'skin-300': withOpacity('--skin-color-300'),
        'skin-200': withOpacity('--skin-color-200'),
        'skin-100': withOpacity('--skin-color-100'),
        'skin-50': withOpacity('--skin-color-50'),
      },
      borderColor: {
        'skin-900': withOpacity('--skin-color-900'),
        'skin-800': withOpacity('--skin-color-800'),
        'skin-700': withOpacity('--skin-color-700'),
        'skin-600': withOpacity('--skin-color-600'),
        'skin-500': withOpacity('--skin-color-500'),
        'skin-400': withOpacity('--skin-color-400'),
        'skin-300': withOpacity('--skin-color-300'),
        'skin-200': withOpacity('--skin-color-200'),
        'skin-100': withOpacity('--skin-color-100'),
      },
      ringColor: {
        'skin-900': withOpacity('--skin-color-900'),
        'skin-800': withOpacity('--skin-color-800'),
        'skin-700': withOpacity('--skin-color-700'),
        'skin-600': withOpacity('--skin-color-600'),
        'skin-500': withOpacity('--skin-color-500'),
        'skin-400': withOpacity('--skin-color-400'),
        'skin-300': withOpacity('--skin-color-300'),
        'skin-200': withOpacity('--skin-color-200'),
        'skin-100': withOpacity('--skin-color-100'),
      },
    },
  },
  variants: {
    extend: {},
  },
  plugins: [require('@tailwindcss/forms'), require('@tailwindcss/typography'), require('daisyui')],
};
