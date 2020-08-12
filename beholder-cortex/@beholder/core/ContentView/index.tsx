import React, {ReactNode, useContext} from 'react';
import {CSSTransition, TransitionGroup} from 'react-transition-group';
import {useRouter} from 'next/router';
import Scrollbar from '../Scrollbar';
import AppContext from '../../app/AppContext';
import AppFooter from '../AppLayout/AppFooter';
import Box from '@material-ui/core/Box';
import {RouteTransition} from '../../app/AppEnums';
import AppContextPropsType from '../../app/AppContextProps';

interface TransitionWrapperProps {
  children: ReactNode;
}

const TransitionWrapper: React.FC<TransitionWrapperProps> = ({children}) => {
  const {rtAnim} = useContext<AppContextPropsType>(AppContext);
  const {pathname} = useRouter();
  if (rtAnim === RouteTransition.NONE) {
    return <>{children}</>;
  }
  return (
    <TransitionGroup appear enter exit>
      <CSSTransition
        key={pathname}
        timeout={{enter: 300, exit: 300}}
        classNames={rtAnim}>
        {children}
      </CSSTransition>
    </TransitionGroup>
  );
};

TransitionWrapper.propTypes = {};

interface ContentViewProps {
  children?: ReactNode;
}

const ContentView: React.FC<ContentViewProps> = (props) => {
  return (
    <Scrollbar>
      <Box
        display='flex'
        flex={1}
        flexDirection='column'
        className='main-content-view'>
        <TransitionWrapper>{props.children}</TransitionWrapper>
      </Box>
      <AppFooter/>
    </Scrollbar>
  );
};

export default ContentView;
