import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomieComponent } from './roomie.component';

describe('RoomieComponent', () => {
  let component: RoomieComponent;
  let fixture: ComponentFixture<RoomieComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RoomieComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(RoomieComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
