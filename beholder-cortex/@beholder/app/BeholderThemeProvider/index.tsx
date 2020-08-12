import React, { useContext, useEffect } from 'react';
import moment from 'moment';
import { ThemeProvider } from '@material-ui/styles';
import { createMuiTheme } from '@material-ui/core/styles';
import { MuiPickersUtilsProvider } from '@material-ui/pickers';

import AppContext from '../AppContext';
import { responsiveFontSizes } from '@material-ui/core';
import { isBreakPointDown } from '../utils';
import { NavStyle, ThemeMode, ThemeStyle } from '../AppEnums';
import { useUrlSearchParams } from 'use-url-search-params';
import AppContextProps from '../AppContextProps';

const BeholderThemeProvider: React.FC<React.ReactNode> = (props) => {
  const {
    theme,
    updateThemeMode,
    changeNavStyle,
    updateThemeStyle,
    updateTheme,
  } = useContext<AppContextProps>(AppContext);

  const initailValue: InitialType = {};
  const types: TypesType = {};
  const [params] = useUrlSearchParams(initailValue, types);

  useEffect(() => {
    const updateQuerySetting = () => {
      if (params.theme_mode) {
        updateThemeMode!(params.theme_mode as ThemeMode);
      }
    };
    updateQuerySetting();
  }, [params.theme_mode, updateThemeMode]);

  useEffect(() => {
    const updateQuerySetting = () => {
      document.body.setAttribute('dir', 'ltr');
    };
    updateQuerySetting();
  });

  useEffect(() => {
    const updateQuerySetting = () => {
      if (params.nav_style) {
        changeNavStyle!(params.nav_style as NavStyle);
      }
    };
    updateQuerySetting();
  }, [changeNavStyle, params.nav_style]);

  useEffect(() => {
    const updateQuerySetting = () => {
      if (params.theme_style) {
        if (params.theme_style === ThemeStyle.MODERN) {
          if (isBreakPointDown('md')) {
            // @ts-ignore
            theme.overrides.MuiCard.root.borderRadius = 20;
            // @ts-ignore
            theme.overrides.MuiToggleButton.root.borderRadius = 20;
          } else {
            // @ts-ignore
            theme.overrides.MuiCard.root.borderRadius = 30;
            // @ts-ignore
            theme.overrides.MuiToggleButton.root.borderRadius = 30;
          }
          // @ts-ignore
          theme.overrides.MuiButton.root.borderRadius = 30;
          // @ts-ignore
          theme.overrides.MuiCardLg.root.borderRadius = 50;
        } else {
          // @ts-ignore
          theme.overrides.MuiCard.root.borderRadius = 4;
          // @ts-ignore
          theme.overrides.MuiToggleButton.root.borderRadius = 4;
          // @ts-ignore
          theme.overrides.MuiButton.root.borderRadius = 4;
          // @ts-ignore
          theme.overrides.MuiCardLg.root.borderRadius = 4;
        }
        updateTheme!(theme);
        if (params.theme_style === ThemeStyle.MODERN || params.theme_style === ThemeStyle.STANDARD) {
          updateThemeStyle!(params.theme_style as ThemeStyle);
        }
      }
    };
    updateQuerySetting();
  }, [params.theme_style, theme, updateTheme, updateThemeStyle]);

  return (
    <ThemeProvider
      theme={responsiveFontSizes(createMuiTheme(theme))}>
      <MuiPickersUtilsProvider utils={moment}>
        {props.children}
      </MuiPickersUtilsProvider>
    </ThemeProvider>
  );
};

export default React.memo(BeholderThemeProvider);
