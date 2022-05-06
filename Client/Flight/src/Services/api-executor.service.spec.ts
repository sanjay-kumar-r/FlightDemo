import { TestBed } from '@angular/core/testing';

import { ApiExecutorService } from './api-executor.service';

describe('ApiExecutorService', () => {
  let service: ApiExecutorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ApiExecutorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
