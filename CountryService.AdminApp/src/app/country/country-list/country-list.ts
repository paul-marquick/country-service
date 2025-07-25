import { Component, inject } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { Country } from '../../../models/country/country';
import { CountryService } from '../../../services/country.service';
import { NgxSpinnerService,  } from "ngx-spinner";
import { Observable } from 'rxjs';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-country-list',
    imports: [AsyncPipe, RouterLink],
    templateUrl: './country-list.html',
    styleUrl: './country-list.css'
})
export class CountryList {

    private countryService: CountryService = inject(CountryService);
    private spinner: NgxSpinnerService = inject(NgxSpinnerService);

    protected countryList: Observable<Country[]>;    

    constructor()
    {
        /** spinner starts on init */
    //    this.spinner.show();

        // Get list of countries.
        this.countryList = this.countryService.getCountryListAsync();
    }           
}
