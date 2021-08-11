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

export interface Navigation {
  title: string;
  icon?: JSX.Element;
  defaultOpen?: boolean;
  current?: boolean;
  url?: string;
  target?: string;
  children?: Navigation[];
}

const navigation: Navigation[] = [
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
  {
    title: 'Services',
    defaultOpen: true,
    children: [
      {
        title: 'Traefik',
        icon: <FontAwesomeIcon icon={faStream} />,
        url: 'https://traefik.{{ host }}/dashboard/',
        target: '_blank',
      },
      {
        title: 'Nexus (EMQ X)',
        icon: <FontAwesomeIcon icon={faStream} />,
        url: 'https://nexus.{{ host }}',
        target: '_blank',
      },
      {
        title: 'Node Red',
        icon: <FontAwesomeIcon icon={faStream} />,
        url: 'https://nodered.{{ host }}',
        target: '_blank',
      },
      {
        title: 'Grafana',
        icon: <FontAwesomeIcon icon={faStream} />,
        url: 'https://grafana.{{ host }}',
        target: '_blank',
      },
    ],
  },
];

export default navigation;
