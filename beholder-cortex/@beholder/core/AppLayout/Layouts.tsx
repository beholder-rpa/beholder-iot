import Standard from './Standard';
import { NavStyle } from '../../app/AppEnums';

interface LayoutsProps {
  [x: string]: any;
}

const Layouts: LayoutsProps = {
  [NavStyle.STANDARD]: Standard,
};
export default Layouts;
