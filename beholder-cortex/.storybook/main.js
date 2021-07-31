const path = require('path')
const tsconfig = require('../tsconfig.json')

const alias = Object.fromEntries( 
  Object.keys(tsconfig.compilerOptions.paths)
    .map(alias => {
      const aliasPath = tsconfig.compilerOptions.paths[alias][0];
      return [ alias.replace(/\/\*$/g, ''), path.resolve(__dirname, `../${aliasPath.replace(/\/\*$/g, '')}`) ];
    })
);

module.exports = {
  stories: [
    "../src/**/*.stories.mdx",
    "../src/**/*.stories.@(js|jsx|ts|tsx)"
  ],
  addons: [
    {
      name: '@storybook/addon-postcss',
      options: {
        postcssLoaderOptions: {
          implementation: require('postcss'),
        },
      },
    },
    "@storybook/addon-links",
    "@storybook/addon-essentials",
    "storybook-addon-next-router"
  ],
  typescript: {
    check: false,
    checkOptions: {},
    reactDocgen: 'react-docgen-typescript',
    reactDocgenTypescriptOptions: {
      shouldExtractLiteralValuesFromEnum: true,
      propFilter: (prop) => (prop.parent ? !/node_modules/.test(prop.parent.fileName) : true),
    },
  },
  webpackFinal: (config) => {
    return {
      ...config,
      resolve: {
        ...config.resolve,
        alias: alias
      },
    };
  },
}
