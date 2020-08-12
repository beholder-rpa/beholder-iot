import { FooterType, LayoutType, NavStyle, RouteTransition, ThemeMode, ThemeStyle } from './AppEnums';
import { PaletteType, Theme, Transitions } from '@material-ui/core';
import { Direction } from '@material-ui/core/styles/createMuiTheme';
import { ZIndex } from '@material-ui/core/styles/zIndex';
import { Spacing } from '@material-ui/core/styles/createSpacing';
import { Mixins } from '@material-ui/core/styles/createMixins';
import { Shape } from '@material-ui/core/styles/shape';
import { Breakpoints } from '@material-ui/core/styles/createBreakpoints';
import { ComponentsProps } from '@material-ui/core/styles/props';
import { Shadows } from '@material-ui/core/styles/shadows';
import { Palette } from '@material-ui/core/styles/createPalette';


interface BeholderPalette extends Palette {
  type: PaletteType,
  background: {
    paper: string,
    default: string,
  },
  primary: {
    light: string;
    main: string;
    dark: string;
    contrastText: string;
  },
  secondary: {
    light: string;
    main: string;
    dark: string;
    contrastText: string;
  },
  sidebar: {
    bgColor: string,
    textColor: string,
  },
  text: {
    primary: string,
    secondary: string,
    disabled: string,
    hint: string,
  },
  common: {
    white: '#fff'
    black: '#fff'
  }

}


export interface BeholderTheme extends Theme {
  direction: Direction,
  palette: BeholderPalette,
  status: {
    danger: string,
  },
  divider: string,
  overrides: {
    MuiTypography: {
      h1: {
        fontSize: number,
      },
      h2: {
        fontSize: number,
      },
      h3: {
        fontSize: number,
      },
      h4: {
        fontSize: number,
      },
      h5: {
        fontSize: number,
      },
      h6: {
        fontSize: number,
      },
      subtitle1: {
        fontSize: number,
      },
      subtitle2: {
        fontSize: number,
      },
      body1: {
        fontSize: number,
      },
      body2: {
        fontSize: number,
      },
    },
    MuiToggleButton: {
      root: {
        borderRadius: number,
      },
    },
    MuiCardLg: {
      root: {
        borderRadius: number,
      },
    },
    MuiCard: {
      root: {
        borderRadius: number,
      },
    },
    MuiButton: {
      root: {
        borderRadius: number,
      },
    },
  },
  spacing: Spacing,
  shape: Shape;
  breakpoints: Breakpoints;
  mixins: Mixins;
  props?: ComponentsProps;
  shadows: Shadows;
  transitions: Transitions;
  zIndex: ZIndex;
  unstable_strictMode?: boolean;
}


export default interface AppContextProps {
  theme: BeholderTheme,
  themeStyle: ThemeStyle,
  themeMode: ThemeMode,
  navStyle: NavStyle,
  layoutType: LayoutType,
  footerType: FooterType,
  rtAnim: RouteTransition,
  footer: boolean,
  primary?: string;
  secondary?: string;
  sidebarColor?: string;
  // routes,
  updateLayoutStyle?: (layoutType: LayoutType) => void;
  updateSidebarColor?: (sidebarColor: string) => void;
  setFooter?: (footer: boolean) => void;
  setFooterType?: (footerType: FooterType) => void;
  updateThemeStyle?: (themeStyle: ThemeStyle) => void;
  updateTheme?: (theme: any) => void;
  updateMode?: (themeMode: ThemeMode) => void;
  updateThemeMode?: (themeMode: ThemeMode) => void;
  updatePrimaryColor?: (primaryColor: string) => void;
  updateSecondaryColor?: (secondaryColor: string) => void;
  changeNavStyle?: (navStyle: NavStyle) => void;
  changeRTAnim?: (routeTransition: RouteTransition) => void
}
