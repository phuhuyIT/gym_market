/**
 * Common Vietnamese banks and their Napas/VietQR BIN codes.
 * The BIN is what img.vietqr.io expects to identify the receiving bank.
 */
export interface VietnamBank {
	bin: string;
	shortName: string;
	name: string;
}

export const VIETNAM_BANKS: VietnamBank[] = [
	{ bin: '970436', shortName: 'Vietcombank', name: 'Ngân hàng TMCP Ngoại Thương Việt Nam' },
	{ bin: '970415', shortName: 'VietinBank', name: 'Ngân hàng TMCP Công Thương Việt Nam' },
	{ bin: '970418', shortName: 'BIDV', name: 'Ngân hàng TMCP Đầu tư và Phát triển Việt Nam' },
	{ bin: '970405', shortName: 'Agribank', name: 'Ngân hàng NN&PTNT Việt Nam' },
	{ bin: '970407', shortName: 'Techcombank', name: 'Ngân hàng TMCP Kỹ Thương Việt Nam' },
	{ bin: '970422', shortName: 'MB Bank', name: 'Ngân hàng TMCP Quân Đội' },
	{ bin: '970416', shortName: 'ACB', name: 'Ngân hàng TMCP Á Châu' },
	{ bin: '970432', shortName: 'VPBank', name: 'Ngân hàng TMCP Việt Nam Thịnh Vượng' },
	{ bin: '970403', shortName: 'Sacombank', name: 'Ngân hàng TMCP Sài Gòn Thương Tín' },
	{ bin: '970423', shortName: 'TPBank', name: 'Ngân hàng TMCP Tiên Phong' },
	{ bin: '970441', shortName: 'VIB', name: 'Ngân hàng TMCP Quốc tế Việt Nam' },
	{ bin: '970437', shortName: 'HDBank', name: 'Ngân hàng TMCP Phát triển TP.HCM' },
	{ bin: '970443', shortName: 'SHB', name: 'Ngân hàng TMCP Sài Gòn - Hà Nội' },
	{ bin: '970448', shortName: 'OCB', name: 'Ngân hàng TMCP Phương Đông' },
	{ bin: '970426', shortName: 'MSB', name: 'Ngân hàng TMCP Hàng Hải' },
	{ bin: '970431', shortName: 'Eximbank', name: 'Ngân hàng TMCP Xuất Nhập Khẩu Việt Nam' },
];
