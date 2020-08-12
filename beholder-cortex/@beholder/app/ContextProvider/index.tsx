import React, { useState } from 'react';
import defaultConfig from './defaultConfig';
import AppContext from '../AppContext';
import PropTypes from 'prop-types';
import { ThemeMode } from '../AppEnums';

const ContextProvider: React.FC<React.ReactNode> = ({ children }) => {
  const [theme, updateTheme] = useState(defaultConfig.theme);
  const [footer, setFooter] = useState(defaultConfig.footer);
  const [footerType, setFooterType] = useState(defaultConfig.footerType);
  const [themeMode, updateMode] = useState(defaultConfig.themeMode);
  const [themeStyle, updateThemeStyle] = useState(defaultConfig.themeStyle);
  const [layoutType, updateLayoutStyle] = useState(defaultConfig.layoutType);
  const [navStyle, changeNavStyle] = useState(defaultConfig.navStyle);
  const [rtAnim, changeRTAnim] = useState(defaultConfig.rtAnim);

  const [primary, updatePrimaryColor] = useState(
    defaultConfig.theme.palette.primary.main,
  );
  const [sidebarColor, updateSidebarColor] = useState(
    defaultConfig.theme.palette.sidebar.bgColor,
  );
  const [secondary, updateSecondaryColor] = useState(
    defaultConfig.theme.palette.secondary.main,
  );

  const updateThemeMode = (themeMode: ThemeMode) => {
    let currentTheme = { ...theme };
    if (themeMode === ThemeMode.DARK) {
      currentTheme.palette.type = ThemeMode.DARK;
      currentTheme.palette.background = {
        paper: '#313541',
        default: '#393D4B',
      };
      currentTheme.palette.text = {
        primary: 'rgba(255, 255, 255, 0.87)',
        secondary: 'rgba(255, 255, 255, 0.67)',
        disabled: 'rgba(255, 255, 255, 0.38)',
        hint: 'rgba(255, 255, 255, 0.38)',
      };
    } else {
      currentTheme.palette.type = ThemeMode.LIGHT;
      currentTheme.palette.background = {
        paper: '#FFFFFF',
        default: '#f3f4f6',
      };
      currentTheme.palette.text = {
        primary: 'rgba(0, 0, 0, 0.87)',
        secondary: 'rgba(0, 0, 0, 0.67)',
        disabled: 'rgba(0, 0, 0, 0.38)',
        hint: 'rgba(0, 0, 0, 0.38)',
      };
    }
    updateTheme(currentTheme);
  };

  return (
    <AppContext.Provider
      value={{
        theme,
        primary,
        secondary,
        themeMode,
        navStyle,
        // routes,
        layoutType,
        updateLayoutStyle,
        rtAnim,
        sidebarColor,
        updateSidebarColor,
        footer,
        footerType,
        setFooter,
        setFooterType,
        themeStyle,
        updateThemeStyle,
        updateTheme,
        updateMode,
        updateThemeMode,
        updatePrimaryColor,
        updateSecondaryColor,
        changeNavStyle,
        changeRTAnim,
      }}>
      {children}
    </AppContext.Provider>
  );
};

export default ContextProvider;

ContextProvider.propTypes = {
  children: PropTypes.node.isRequired,
};
