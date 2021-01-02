import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { AuditedEntityDto } from '@abp/ng.core';

export interface AuthorDto extends AuditedEntityDto<string> {
  name?: string;
  birthDate?: string;
  shortBio?: string;
}

export interface CreateAuthorDto {
  name: string;
  birthDate: string;
  shortBio?: string;
}

export interface GetAuthorListDto extends PagedAndSortedResultRequestDto {
  filter?: string;
}

export interface UpdateAuthorDto {
  name: string;
  birthDate: string;
  shortBio?: string;
}
