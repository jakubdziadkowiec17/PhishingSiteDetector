export interface ListPageDTO<T> {
  items: T[];
  count: number;
  pageNumber: number;
}