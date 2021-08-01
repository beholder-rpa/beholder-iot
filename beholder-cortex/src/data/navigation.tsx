import {
  faKeyboard,
  faMouse,
  faTachometerAlt,
  faAirFreshener,
  faSchool,
  faTools,
  faStream,
} from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const navigation = [
  { title: 'Dashboard', icon: <FontAwesomeIcon icon={faTachometerAlt} />, current: true, url: '/' },
  {
    title: 'Event Handlers',
    icon: <FontAwesomeIcon icon={faAirFreshener} />,
    defaultOpen: true,
    children: [
      {
        title: 'Keyboard',
        icon: <FontAwesomeIcon icon={faKeyboard} />,
        url: '/event-handlers/keyboard',
      },
      {
        title: 'Mouse',
        icon: <FontAwesomeIcon icon={faMouse} />,
        url: '/event-handlers/mouse',
      },
    ],
  },
  {
    title: 'ML Workspace',
    icon: <FontAwesomeIcon icon={faSchool} />,
    url: '/ml-workspace',
  },
  {
    title: 'Tools',
    icon: <FontAwesomeIcon icon={faTools} />,
    defaultOpen: true,
    children: [
      {
        title: 'Virtual Controls',
        icon: <FontAwesomeIcon icon={faKeyboard} />,
        url: '/tools/virtualcontrols',
      },
      {
        title: 'Status',
        icon: <FontAwesomeIcon icon={faStream} />,
        url: '/tools/status',
      },
    ],
  },
];

export default navigation;
