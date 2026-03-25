import { Component, input } from '@angular/core';
import { Property } from '../../../types/property';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-property-card',
  imports: [RouterLink],
  templateUrl: './property-card.html',
  styleUrl: './property-card.css',
})
export class PropertyCard {
  //member will be an input signal property received from memberlist
  property = input.required<Property>(); 
}
