select distinct ohcuno as CUSTNO, oaname as CUSTNAM, ohcope as ADVTSR, olshpm as DESIGN, 
	olprdc as ITEMNO, olmotc as SHPMTH, aya0dt as TGTSDT, oldsdt as ACTSDT,
    olrdvd as TGTDLD, pdz3pddt as ACTDLD, pdz3podn as RCVDBY, thcstrcn as TRACK, 
	ohoref as PONUM, ohorno as ORDNO, olline as ORLINE, updesc as CSRNAM, substring(ulcpar, 7) as CSREML,
 	case 
		when olords = 10 then '00500' 
 		when olords = 30 then '07000' 
		when olords = 45 and thcstrcn like 'delivered%' then '09000' 
		when olords = 45 and thz3delv = 'Y' then '09000' 
		when olords = 45 and thz3delv = '' then '08000' 
		when olords = 45 and thz3delv = 'N' then '08000' 
 		when olords = 60 and thz3delv = 'Y' then '09000' 
		when olords = 60 and thcstrcn like 'delivered%' then '09000' 
		when olords = 60 and thz3delv = '' then '08000' 
		when olords = 60 and thz3delv = 'N' then '08000' 
 		when olords = 20 and ayavst = '10' and aybrnb = 0 and aybpnb = 0 then '01000' 
		when olords = 20 and ayavst = '20' and aybrnb = 0 and aybpnb = 0 then '01000' 
		when olords = 20 and ayavst = '20' and aybrnb = 0 and aybpnb = 10 then '01000' 
		when olords = 20 and ayavst = '40' and aybrnb = 0 and aybpnb = 10 then '02000' 
		when olords = 20 and ayavst = '40' and aybrnb = 1 and aybpnb = 10 then '03000' 
		when olords = 20 and ayavst = '40' and aybrnb = 1 and aybpnb = 15 then '03000' 
		when olords = 20 and ayavst = '40' and aybrnb = 2 and aybpnb = 15 then '04000' 
		when olords = 20 and ayavst = '40' and aybrnb = 2 and aybpnb = 18 then '04000' 
		when olords = 20 and ayavst = '40' and aybrnb = 3 and aybpnb = 18 then '05000' 
		when olords = 20 and ayavst = '40' and aybrnb = 3 and aybpnb = 20 then '05000' 
		when olords = 20 and ayavst = '40' and aybrnb = 4 and aybpnb = 20 then '06000' 
		when olords = 20 and ayavst = '40' and aybrnb = 4 and aybpnb = 30 then '06000' 
		when olords = 20 and ayavst = '60' and aybrnb = 5 and aybpnb = 30 then '07000' 
		when olords = 20 and ayavst = '60' and aybrnb = 6 and aybpnb = 30 then '07000' 
		when olords = 45 and ayavst = '60' and aybrnb = 5 and aybpnb = 30 then '08000' 
		else 'Undefined' 
	end STATUS
 from  vt2662aftt.sroorshe SOHdr
	left join vt2662aftt.sroorspl Oline on SOHdr.ohorno = Oline.olorno and Oline.olshpm <> 'FREIGHT'
	left join vt2662aftt.mfmohr MFG on Oline.olorno = MFG.aybmnb and Oline.olline = MFG.aywdnb
	left join vt2662aftt.sroorsa OAddr on Oline.olorno = OAddr.oaorno and Oline.olline = OAddr.oaline
	left join vt2662aftt.sronoi SInfo on SOHdr.ohcuno = SInfo.nonum
	left join  vt2662aftt.srousp ibsuser on SInfo.noshan = ibsuser.uphand
	left join vt2662aftt.srocll on ibsuser.upuser = uluser
	left join VT2662aftt.Z3OPTRH on  Oline.olplno = thplno and Oline.olorno = thorno and thz3delv = 'Y'
	left join VT2662aftt.Z3OPTRpD on thcstrcn = pdcstrcn
where ohcuno = '{CUST_NO}' and ohorno = {ORDER_NO}