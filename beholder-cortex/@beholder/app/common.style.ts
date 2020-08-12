import makeStyles from '@material-ui/core/styles/makeStyles';
import { Fonts } from './AppEnums';

const useStyles = makeStyles(() => ({
  '@global': {
    // for global styles
    '.MuiLink-root': {
      fontFamily: Fonts.REGULAR,
    },
  },
}));

export default useStyles;
