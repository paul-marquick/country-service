import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CountryService } from '../../../services/country.service';
import { AsyncPipe, JsonPipe } from '@angular/common';
import { Observable } from 'rxjs';
import { Country } from '../../../models/country/country';
import { ReactiveFormsModule, FormControl, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { CountryNameExistsValidator } from '../../../validators/countryNameExistsValidator';
import { forbiddenNameValidator } from '../../../validators/forbiddenNameValidator';

@Component({
    selector: 'app-country-edit',
    imports: [AsyncPipe, JsonPipe, ReactiveFormsModule],
    templateUrl: './country-edit.html',
    styleUrl: './country-edit.css'
})
export class CountryEdit {

    private countryService: CountryService = inject(CountryService);
    private route: ActivatedRoute = inject(ActivatedRoute);
    private countryNameExistsValidator: CountryNameExistsValidator = inject(CountryNameExistsValidator);
    
    protected country$: Observable<Country | undefined>;
    protected countryForm: FormGroup;

    constructor() {
        
        // Get the iso2 path parameter.
        const iso2: string = this.route.snapshot.params["iso2"];

        // Get the country.
        this.country$ = this.countryService.getCountryByIso2(iso2);

        //TODO: handle errors.

        this.countryForm = new FormGroup({    
            name: new FormControl('', { validators: [Validators.required, forbiddenNameValidator], asyncValidators: [this.countryNameExistsValidator.checkCountryNameExists(iso2)] }),    
            iso2: new FormControl('', { validators: [Validators.required, Validators.minLength(2), Validators.maxLength(2)]}),    
            iso3: new FormControl('', { validators: [Validators.required, Validators.minLength(3), Validators.maxLength(3)]}),    
            isoNumber: new FormControl('', { validators: [Validators.required]}),   
        });

        this.country$.subscribe({next: country => {
            this.countryForm.controls["name"].setValue(country?.name);
            this.countryForm.controls["iso2"].setValue(country?.iso2);
            this.countryForm.controls["iso3"].setValue(country?.iso3);
            this.countryForm.controls["isoNumber"].setValue(country?.isoNumber);
        }, 
        error: error => console.log('oops', error)}); 
    }    

    protected handleSubmit() {

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
