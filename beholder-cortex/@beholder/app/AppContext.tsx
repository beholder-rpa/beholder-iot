import React from 'react';
import AppContextProps from './AppContextProps';
import defaultConfig from './ContextProvider/defaultConfig';

export default React.createContext<AppContextProps>(defaultConfig);
