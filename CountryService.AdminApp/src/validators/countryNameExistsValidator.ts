import { Injectable, inject } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, ValidationErrors } from '@angular/forms';
import { Observable, of } from 'rxjs';
import { debounceTime, map, catchError, switchMap } from 'rxjs/operators';
import { CountryService } from '../services/country.service';

// Custom async validator.
// Checks if the country name is already in the database, if iso2 is passed in then that one is ignored.
@Injectable({ providedIn: 'root' })
export class CountryNameExistsValidator {

    private countryService: CountryService = inject(CountryService);

    checkCountryNameExists(iso2: string | null): AsyncValidatorFn {
        
        return (control: AbstractControl): Observable<ValidationErrors | null> => {

            // Return null for empty values (valid by default).
            if (!control.value) {
                return of(null);
            }

            return of(control.value).pipe(

                // Delay processing to debounce user input.
                debounceTime(500),

                // Cancel previous requests and switch to the latest.
                switchMap((name) =>
                    this.countryService.checkIfCountryNameExists(name, iso2).pipe(

                        // If the API returns data, emit { countryNameExists: true }; otherwise null.
                        map((response) => (response ? { countryNameExists: true } : null)),

                        // Handle errors (e.g., network issues).
                        catchError(() => of(null))
                    )
                )
            );
        };
    }
}
