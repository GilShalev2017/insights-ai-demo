import { Component } from '@angular/core';
import { PeopleService } from '../services/people.service';

@Component({
  selector: 'app-named-entities',
  templateUrl: './named-entities.component.html',
  styleUrls: ['./named-entities.component.css']
})
export class NamedEntitiesComponent {
  
  constructor(public peopleService: PeopleService) {
  
    
  }
}
