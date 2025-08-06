import { Routes } from '@angular/router';
import { Home } from './home/home';
import { CountryList } from './country/country-list/country-list';
import { CountryEdit } from "./country/country-edit/country-edit"

export const routes: Routes = [
    {
        path: '',
        component: Home,
        title: 'Home page',
    },
    {
        path: 'country',
        component: CountryList,
        title: 'Countries',
    },
    {
        path: 'country/edit/:iso2',
        component: CountryEdit,
        title: 'Edit Country',
    }
];
