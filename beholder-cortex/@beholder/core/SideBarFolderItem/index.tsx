import React from 'react';
import AppNavLink from '../AppNavLink';
import Box from '@material-ui/core/Box';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import ListItem from '@material-ui/core/ListItem';
import { makeStyles } from '@material-ui/core/styles';
import { Fonts } from '../../app/AppEnums';
import Icon from '@material-ui/core/Icon';
import { BeholderTheme } from '../../app/AppContextProps';

interface WrappedIconProps {
    props?: any;
}

const WrappedIcon: React.FC<WrappedIconProps> = (props) => <Icon {...props} />;
// @ts-ignore
WrappedIcon.muiName = 'Icon';

interface AppsSideBarFolderItemProps {
    item: {
        id: number | string;
        name: string;
        icon: any
    };
    path: string
}

const AppsSideBarFolderItem: React.FC<AppsSideBarFolderItemProps> = ({ item, path }) => {
    const useStyle = makeStyles((theme: BeholderTheme) => ({
        listItem: {
            paddingLeft: '10px',
            paddingRight: '0',
            paddingTop: '5px',
            paddingBottom: '5px',

            '& .MuiListItemText-root': {
                [theme.breakpoints.down('lg')]: {
                    marginTop: 0,
                    marginBottom: 0,
                },
            },

            '& .MuiTypography-body1': {
                fontSize: '16px',
                color: '#A8A8A8',
                [theme.breakpoints.up('xl')]: {
                    fontSize: '18px',
                },
            },

            '& svg': {
                fontSize: '18px',
                color: '#A8A8A8',
                [theme.breakpoints.up('xl')]: {
                    fontSize: '20px',
                },
            },

            '&:hover,&:focus,&.active': {
                backgroundColor: 'transparent',
                color: theme.palette.primary.main,

                '& svg': {
                    fontSize: '18px',
                    color: theme.palette.primary.main,
                    [theme.breakpoints.up('xl')]: {
                        fontSize: '20px',
                    },
                },

                '& .MuiTypography-root': {
                    color: theme.palette.primary.main,
                },
            },

            '&.active': {
                color: theme.palette.primary.main,
                fontFamily: Fonts.LIGHT,

                '& svg, & .MuiTypography-root': {
                    fontFamily: Fonts.LIGHT,
                    color: theme.palette.primary.main,
                },
            },
        },
        listItemIcon: {
            minWidth: 10,
            paddingTop: 4,
        },
        listItemText: {
            fontFamily: 'inherit',
        },
    }));

    const classes = useStyle();
    return (
        <ListItem
            button
            key={item.id}
            to={path}
            component={AppNavLink}
            className={classes.listItem}
            activeClassName='active'>
            <Box component='span' mr={{ xs: 4, xl: 7 }}>
                <ListItemIcon className={classes.listItemIcon}>
                    <WrappedIcon>{item.icon}</WrappedIcon>
                </ListItemIcon>
            </Box>
            <ListItemText primary={item.name} className={classes.listItemText} />
        </ListItem>
    );
};

export default AppsSideBarFolderItem;
