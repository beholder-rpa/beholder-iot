import { faKeyboard, faMouse, faTachometerAlt, faAirFreshener, faSchool } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

const navigation = [
  { title: 'Dashboard', icon: <FontAwesomeIcon icon={faTachometerAlt} size="2x" />, current: true, url: '/' },
  {
    title: 'Event Handlers',
    icon: <FontAwesomeIcon icon={faAirFreshener} size="2x" />,
    defaultOpen: true,
    children: [
      {
        title: 'Keyboard',
        icon: <FontAwesomeIcon icon={faKeyboard} size="2x" />,
        url: '/event-handlers/keyboard',
      },
      {
        title: 'Mouse',
        icon: <FontAwesomeIcon icon={faMouse} size="2x" />,
        url: '/event-handlers/mouse',
      },
    ],
  },
  {
    title: 'ML Workspace',
    icon: <FontAwesomeIcon icon={faSchool} size="2x" />,
    url: '/ml-workspace',
  },
];

export default navigation;
