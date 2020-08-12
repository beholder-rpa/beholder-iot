import React, { ReactNode, useEffect, useRef } from 'react';
import PerfectScrollbar from 'react-perfect-scrollbar';
import { withRouter } from 'next/router';

interface ScrollbarProps {

  children: ReactNode
  className: string,
  router: any,

  [x: string]: any
}

const Scrollbar: React.FC<ScrollbarProps> = React.forwardRef((props, ref) => {
  const {
    children,
    className,
    router,
    ...others
  } = props;
  let _scrollBarRef = useRef<HTMLElement>(null);
  const { pathname } = router;

  useEffect(() => {
    if (_scrollBarRef) {
      // @ts-ignore
      _scrollBarRef._container.scrollTop = 0;
    }
  }, [_scrollBarRef, pathname]);

  return (
    <PerfectScrollbar
      ref={(ref) => {
        // @ts-ignore
        _scrollBarRef = ref;
      }}
      {...others}
      className={className}>
      {children}
    </PerfectScrollbar>
  );
});

export default withRouter(Scrollbar);
