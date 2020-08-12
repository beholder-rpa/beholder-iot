import {useTheme} from '@material-ui/core/styles';
import useMediaQuery from '@material-ui/core/useMediaQuery';

export const isBreakPointDown = (key: 'xs' | 'sm' | 'md' | 'lg' | 'xl') => {
  const theme = useTheme();
  return useMediaQuery(theme.breakpoints.down(key));
};

export const getBreakPointsValue = (valueSet: any, breakpoint: string) => {
  if (typeof valueSet === 'number') return valueSet;
  switch (breakpoint) {
    case 'xs':
      return valueSet.xs;
    case 'sm':
      return valueSet.sm || valueSet.xs;
    case 'md':
      return valueSet.md || valueSet.sm || valueSet.xs;
    case 'lg':
      return valueSet.lg || valueSet.md || valueSet.sm || valueSet.xs;
    default:
      return (
        valueSet.xl || valueSet.lg || valueSet.md || valueSet.sm || valueSet.xs
      );
  }
};

