import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import CountryService from '../../country.service';
import { AsyncPipe, JsonPipe } from '@angular/common';
import { Observable } from 'rxjs';
import Country from '../../country';
import { ReactiveFormsModule, FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'app-country-edit',
    imports: [AsyncPipe, JsonPipe, ReactiveFormsModule],
    templateUrl: './country-edit.html',
    styleUrl: './country-edit.css'
})
export class CountryEdit {

    private countryService: CountryService = inject(CountryService);
    private route: ActivatedRoute = inject(ActivatedRoute);
    
    protected country$: Observable<Country | undefined>;
    protected countryForm: FormGroup;

    constructor() {
        
        const iso2: string = this.route.snapshot.params["iso2"];
        this.country$ = this.countryService.getCountryByIso2(iso2);

        this.countryForm = new FormGroup({    
            name: new FormControl('', { validators: [Validators.required]}),    
            iso2: new FormControl('', { validators: [Validators.required, Validators.minLength(2), Validators.maxLength(2)]}),    
            iso3: new FormControl('', { validators: [Validators.required, Validators.minLength(3), Validators.maxLength(3)]}),    
            isoNumber: new FormControl('', { validators: [Validators.required]}),   
        });

        this.country$.subscribe(country => {
            this.countryForm.controls["name"].setValue(country?.name);
            this.countryForm.controls["iso2"].setValue(country?.iso2);
            this.countryForm.controls["iso3"].setValue(country?.iso3);
            this.countryForm.controls["isoNumber"].setValue(country?.isoNumber);
        });        
    }

    handleSubmit() {

        if (this.countryForm.valid) {
            console.log("form is valid.");
            alert(this.countryForm.value.name + ' | ' + this.countryForm.value.iso2);
        }
        else {
            console.log("form is invalid.");
            console.log("don't call web service.");
        }
    }
}
