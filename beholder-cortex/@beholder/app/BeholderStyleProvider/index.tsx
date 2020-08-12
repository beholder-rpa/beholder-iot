import React from 'react';
import {create} from 'jss';
import {jssPreset, StylesProvider} from '@material-ui/core/styles';
import PropTypes from 'prop-types';

// Configure JSS
const jss = create({plugins: [...jssPreset().plugins]});

const BeholderStyleProvider: React.FC<React.ReactNode> = (props) => {
  return <StylesProvider jss={jss}>{props.children}</StylesProvider>;
};
export default BeholderStyleProvider;

BeholderStyleProvider.propTypes = {
  children: PropTypes.node.isRequired,
};
