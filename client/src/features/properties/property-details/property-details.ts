import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MemberService } from '../../../core/services/member-service';
import { PropertyService } from '../../../core/services/property-service';
import { SessionService } from '../../../core/services/session-service';
import { Property } from '../../../types/property';
import { ActivatedRoute, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { DatePipe, JsonPipe } from '@angular/common';
import { AgePipe } from '../../../core/pipes/age-pipe';
import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { TextInput } from "../../../shared/text-input/text-input";
import { ToastService } from '../../../core/services/toast-service';
import { min } from 'rxjs';

@Component({
  selector: 'app-property-details',
  imports: [TimeAgoPipe, TextInput, ReactiveFormsModule],
  templateUrl: './property-details.html',
  styleUrl: './property-details.css',
})
export class PropertyDetails implements OnInit {
  protected session = inject(SessionService)
  private route = inject(ActivatedRoute);
  protected propertyService = inject(PropertyService)
  protected toast = inject(ToastService)
  protected currentStep = signal(1);

  // protected property$?: Observable<Property>;
  // protected property = signal<Property | null>(null); 

  protected fb = inject(FormBuilder)
  protected editablePropertyForm: FormGroup;
  protected addressForm: FormGroup;


  constructor() {
    this.editablePropertyForm = this.fb.nonNullable.group({
      lotNumber: new FormControl(''),
      squareFeet: new FormControl(0,[Validators.required, Validators.min(1)]),
      bedrooms: new FormControl(0, [Validators.required, Validators.min(1)]),
      bathrooms: new FormControl(0,[Validators.required, Validators.min(1)]),
      isRented: new FormControl(false, Validators.required),
      assignedParking: new FormControl('')
    });
    this.addressForm = this.fb.nonNullable.group({
      address: new FormControl('', Validators.required),
      unit: new FormControl(''),
      city: new FormControl('', Validators.required),
      state: new FormControl('', Validators.required),
      zipCode: new FormControl('', Validators.required),
      country: new FormControl('USA', Validators.required),
      mailAddress: new FormControl('', Validators.required),
      mailUnit: new FormControl(''),
      mailCity: new FormControl('', Validators.required),
      mailState: new FormControl('', Validators.required),
      mailZipCode: new FormControl('', Validators.required),
      mailCountry: new FormControl('USA', Validators.required),
      isSameAddress: new FormControl(false, Validators.required),
    });
  }
  ngOnInit(): void {
    // here we are using route.parent as details is looking for data from profile
    this.route.parent?.data.subscribe(data => {
      this.propertyService.property.set(data['property']);
       this.initializeValues(); 
    });
 
    this.addressForm.get('isSameAddress')?.valueChanges.subscribe(isSame => {
      this.mailingAddressHandler(isSame);
    });
  }

  mailingAddressHandler(isSame: boolean) {
    if (isSame) {
      const p = this.propertyService.property();
      if (!p) return;
      this.addressForm.patchValue({
        mailAddress: p.address,
        mailUnit: p.unit,
        mailCity: p.city,
        mailState: p.state,
        mailZipCode: p.zipCode,
        mailCountry: 'USA'
      }); 
    }
  }

  initializeValues() {
    this.currentStep.set(1);

    this.addressForm.patchValue({
      address: this.propertyService.property()?.address,
      unit: this.propertyService.property()?.unit,
      city: this.propertyService.property()?.city,
      state: this.propertyService.property()?.state,
      zipCode: this.propertyService.property()?.zipCode,
      country: this.propertyService.property()?.country,
      mailAddress: this.propertyService.property()?.mailAddress,
      mailUnit: this.propertyService.property()?.mailUnit,
      mailCity: this.propertyService.property()?.mailCity,
      mailState: this.propertyService.property()?.mailState,
      mailZipCode: this.propertyService.property()?.mailZipCode,
      mailCountry: this.propertyService.property()?.mailCountry,
      isSameAddress: this.propertyService.property()?.isSameAddress ?? false
    });
    this.editablePropertyForm.patchValue({
      lotNumber: this.propertyService.property()?.lotNumber,
      squareFeet: this.propertyService.property()?.squareFeet,
      bedrooms: this.propertyService.property()?.bedrooms,
      bathrooms: this.propertyService.property()?.bathrooms,
      isRented: this.propertyService.property()?.isRented ?? false,
      assignedParking: this.propertyService.property()?.assignedParking
    });
  }
  cancelSave() {
    this.currentStep.set(1);
    if (this.propertyService.property()) {
      this.addressForm.reset();
      this.editablePropertyForm.reset()
      this.initializeValues() 
    }

    this.propertyService.editMode.set(!this.propertyService.editMode())
  }
  Update() {
    console.log('UPDATE CALLED', this.editablePropertyForm.value, this.addressForm.value);
    if (!this.editablePropertyForm.valid || !this.addressForm.valid) return;
    if (this.editablePropertyForm.valid && this.addressForm.valid) {
      const updatedProperty = { ...this.propertyService.property(), ...this.editablePropertyForm.value, ...this.addressForm.value };
      // console.log('update property values', updatedProperty)
      this.propertyService.updateProperty(updatedProperty).subscribe({
        next: () => {
          // console.log(result)
          this.toast.success("Successfully updated property");
          this.propertyService.editMode.set(false);
          this.propertyService.property.set(updatedProperty as Property); //as Property removes the typesript 
          this.addressForm.reset(updatedProperty);
          this.editablePropertyForm.reset(updatedProperty)  
        },
        error: err => {
          // console.log('Error while updating property ', err);
          this.toast.error(`Error while updating property ${this.editablePropertyForm.controls['address'].value}. ${err}`)
        }
      });
    }

  }

  nextStep() {
    if (this.addressForm.valid) {
      this.currentStep.update(prevStep => prevStep + 1);
    }
  }

  prevStep() {
    this.currentStep.update(prevStep => prevStep - 1);
  }

}
