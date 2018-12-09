// struct to store page info (in api response header)
export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

// generic implementation of storing info in above interface

export class PaginatedResult<T> {
    // for storing users or messages...
    result: T;
    // for paging info
    pagination: Pagination;

}

// now we can use it in our user.services.ts
