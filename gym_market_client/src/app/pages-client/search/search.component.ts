import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
	selector: 'app-search',
	standalone: true,
	imports: [RouterLink],
	templateUrl: './search.component.html',
	styleUrl: './search.component.scss',
})
export class SearchComponent {
	trainers: any;

	ngOnInit() {
		this.trainers = [
			{
				image: 'https://resizing.flixster.com/Yf2fuddkUv1c_ZNHjsHW1o3icCM=/fit-in/352x330/v2/https://resizing.flixster.com/-XZAfHZM39UwaGJIFWKAE8fS0ak=/v3/t/assets/p15319157_i_h9_ac.jpg',
				name: 'Ronnie Coleman',
				description:
					'Một trong những vận động viên thể hình nổi tiếng nhất, Ronnie Coleman đã giành được 8 lần danh hiệu Mr. Olympia (1998–2005). Anh được biết đến với cơ bắp khổng lồ và sự chăm chỉ không ngừng',
				cert: 'IFBB Pro Bodybuilder',
			},
			{
				image: 'https://cdn.britannica.com/11/222411-050-D3D66895/American-politician-actor-athlete-Arnold-Schwarzenegger-2016.jpg?w=400&h=300&c=crop',
				name: 'Arnold Schwarzenegger',
				description:
					' Là huyền thoại của làng thể hình, Arnold đã giành 7 danh hiệu Mr. Olympia và đóng vai trò quan trọng trong việc quảng bá môn thể thao này trên toàn thế giới. Sau đó, ông còn nổi danh trong lĩnh vực điện ảnh và chính trị.',
				cert: 'IFBB Pro Bodybuilder',
			},
			{
				image: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQ0-1TBUPp2BzBQ9uodKD_ofhrof7PQ38nDNEj01dnaoU9N1Z3_vZ3waZbPLZoU2jChghg&usqp=CAU',
				name: 'Phil Heath',
				description:
					'Được gọi là "The Gift," Phil Heath đã giành được 7 danh hiệu Mr. Olympia từ 2011 đến 2017, nổi bật với cơ bắp đối xứng và sự chuyên nghiệp.',
				cert: 'IFBB Pro Bodybuilder',
			},
			{
				image: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQy5I0J2yMfMDQtYDRofKYXwpQTSoEuZS9V4HTgkrJb0mKr83WniRAcnPpd3HWsiqiEvmQ&usqp=CAU',
				name: 'Kai Greene',
				description:
					'Một trong những vận động viên thể hình ấn tượng với phong cách trình diễn đặc biệt, Kai Greene đã về nhì nhiều lần tại giải Mr. Olympia.',
				cert: 'IFBB Pro Bodybuilder',
			},
			{
				image: 'https://i.ytimg.com/vi/y7UpMBtWTYk/hqdefault.jpg?sqp=-oaymwEmCOADEOgC8quKqQMa8AEB-AH-BIAC6AKKAgwIABABGHIgUyhCMA8=&rs=AOn4CLArKNyncTaC6KZaJYEPryBAVMcPKA',
				name: 'Jay Cutler',
				description:
					'Jay Cutler đã giành được 4 danh hiệu Mr. Olympia (2006, 2007, 2009, 2010). Anh nổi tiếng với sự kiên trì và khả năng quay trở lại sau thất bại.',
				cert: 'IFBB Pro Bodybuilder',
			},
			{
				image: 'https://i.etsystatic.com/51386728/r/il/650d02/5889723726/il_570xN.5889723726_lkb9.jpg',
				name: 'Dorian Yates',
				description:
					'Dorian Yates là một huyền thoại người Anh, nổi bật với vóc dáng to lớn và kỹ thuật tập luyện tiên tiến. Ông đã giành được 6 danh hiệu Mr. Olympia (1992–1997).',
				cert: 'IFBB Pro Bodybuilder',
			},
			{
				image: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTGqe8Ek4-Sj5AQomOHw1jSwlQbryOD7SnPbGMhPP-QrWfuVbKi7z0K9o7zIimiIhcLXp0&usqp=CAU',
				name: 'Hadi Choopan',
				description:
					'Hadi là một vận động viên người Iran nổi bật, giành nhiều chiến thắng tại các giải thể hình quốc tế và có màn ra mắt ấn tượng tại Mr. Olympia.',
				cert: 'IFBB Pro Bodybuilder',
			},
			{
				image: 'https://i.ytimg.com/vi/aY0E2sxxKd0/sddefault.jpg',
				name: 'Dexter Jackson',
				description:
					'"The Blade" Dexter Jackson đã giành Mr. Olympia vào năm 2008 và là vận động viên thể hình giữ kỷ lục về số lần thi đấu trong sự nghiệp với hơn 25 năm chuyên nghiệp.',
				cert: 'IFBB Pro Bodybuilder',
			},
			{
				image: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRYSLZvrWzRDYOGqdHX-Fv3_LqQ-6Zb1b6Amg&s',
				name: 'Chris Bumstead',
				description:
					'Chris là nhà vô địch Classic Physique Mr. Olympia từ năm 2019, được công nhận nhờ sự cân đối hoàn hảo và phong cách cổ điển.',
				cert: 'IFBB Pro Bodybuilder',
			},
			{
				image: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSo12QiQOaENODAjadnvf1UISt7ANUSx90ZfA&s',
				name: 'Lazar Angelov',
				description:
					'Lazar là một vận động viên thể hình và huấn luyện viên nổi tiếng người Bulgaria, nổi bật với cơ bụng "6 múi" hoàn hảo và là biểu tượng thể hình trên toàn thế giới.',
				cert: 'IFBB Pro Bodybuilder',
			},
		];
	}

	onSubmit() {
		console.log(123);
	}
}
