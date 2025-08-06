import { AbstractControl, ValidationErrors } from '@angular/forms';

// Custom sync validator.
// Stops user entering 'foo' or 'bar' in as the country name.
export const forbiddenNameValidator = (control: AbstractControl): ValidationErrors | null => 
{
    const names = ["foo", "bar"];

    return names.includes(control.value) ? { forbiddenName: "Name is not allowed"} : null;
};
