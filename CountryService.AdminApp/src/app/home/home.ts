import { Component } from '@angular/core';
import { environment } from '../../environments/environment';
import { Environment } from '../../models/environment';

@Component({
    selector: 'app-home',
    imports: [],
    templateUrl: './home.html',
    styleUrl: './home.css'
})
export class Home {

    protected env: Environment = environment;
}
